const api_url_base = "http://34.56.66.163"; 

export default async function apiFetch(endpoint, options = {})
{
    const user = JSON.parse(localStorage.getItem("user"));
    const accessToken = user?.accessToken;
    const refreshToken = user?.refreshToken;

    const headers = {
        "Content-Type": "application/json",
        ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
        ...(options.headers || {})
    };

    let response = await fetch(`${api_url_base}${endpoint}`, {
        ...options,
        headers,
        credentials: "include"
    });

    if (response.ok) return response;

    if (response.status === 401 && refreshToken)
    {
        const refreshed = await refreshTokens(refreshToken);
        if (refreshed)
        {
            const newUser = JSON.parse(localStorage.getItem("user"));
            const newAccessToken = newUser?.accessToken;

            const newHeaders = {
                ...headers,
                Authorization: `${newAccessToken}`
            };

            response = await fetch(`${api_url_base}${endpoint}`, {
                ...options,
                headers: newHeaders,
                credentials: "include"
            });

            if (response.ok) return response;
        }
    }

    if (response.status === 409) {
      const errorData = await response.json().catch(() => ({}));
      console.warn("⚠️ Conflict:", errorData?.message || "Conflict");
      throw new Error(errorData?.message || "Conflict");
    }

    if (response.status === 400 || response.status === 401 || response.status === 403 || response.status === 404) {
      const errorData = await response.json().catch(() => ({}));
      throw new Error(errorData?.reason || errorData?.message || `HTTP ${response.status}`);
    }

    throw new Error("Failed to fetch API");
    }

async function refreshTokens(refreshToken) {
  try {
    const response = await fetch(`${api_url_base}/api/Token/refresh`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ refreshToken }),
    });

    if (!response.ok) {
      console.warn("Refresh token invalid or expired");
      localStorage.removeItem("user");
      return false;
    }

    const data = await response.json();

    const current = JSON.parse(localStorage.getItem("user"));
    const updated = {
      ...current,
      accessToken: data.accessToken,
      refreshToken: data.refreshToken,
    };
    localStorage.setItem("user", JSON.stringify(updated));

    console.log("🔁 Token refreshed");
    return true;
  } catch (err) {
    console.error("Token refresh failed:", err);
    return false;
  }
}