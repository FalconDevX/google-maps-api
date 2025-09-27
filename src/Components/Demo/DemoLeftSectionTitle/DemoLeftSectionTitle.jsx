import { useEffect, useState } from "react"

const DemoLeftSectionTitle = () => {
    const [username, setUsername] = useState("");

    useEffect(() => {
        const userData = localStorage.getItem("user");
        if (userData) {
            const user = JSON.parse(userData);
            setUsername(user.username);
        }
        else
        {
            setUsername("Anonimous");
        }
    }, []);
    return (

    <h1>👋 Witaj{username ? `, ${username}` : ""}!</h1>
  )
}

export default DemoLeftSectionTitle