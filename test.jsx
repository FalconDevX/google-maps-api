import React, { useCallback, useEffect, useMemo, useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import {
  Heart,
  X as Close,
  Bookmark,
  MapPin,
  Globe2,
  Leaf,
  Wind,
  Settings,
  ChevronLeft,
  ChevronRight,
  Sparkles,
  ThumbsUp,
  ThumbsDown,
} from "lucide-react";

// ---------------------------------------------
// Demo data – feel free to swap with your API
// ---------------------------------------------
const PLACES = [
  {
    id: "1",
    name: "Morskie Oko",
    city: "Tatry",
    country: "Polska",
    countryFlag: "🇵🇱",
    color: "emerald",
    image:
      "https://images.unsplash.com/photo-1501785888041-af3ef285b470?q=80&w=1640&auto=format&fit=crop",
    desc:
      "Górskie jezioro otoczone świerkami. Szlaki o niskim natężeniu ruchu poza sezonem.",
    tags: ["jezioro", "góry", "cisza"],
    co2SavedKg: 2.1,
    calmBoost: 34,
  },
  {
    id: "2",
    name: "Białowieża",
    city: "Puszcza Białowieska",
    country: "Polska",
    countryFlag: "🇵🇱",
    color: "teal",
    image:
      "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?q=80&w=1640&auto=format&fit=crop",
    desc: "Miękki mech, stare dęby i mikro-trasy bez tłumów. Rezerwat biosfery.",
    tags: ["las", "rezerwat", "zielono"],
    co2SavedKg: 3.4,
    calmBoost: 41,
  },
  {
    id: "3",
    name: "Hallstatt",
    city: "Alpy",
    country: "Austria",
    countryFlag: "🇦🇹",
    color: "sky",
    image:
      "https://images.unsplash.com/photo-1505935428862-770b6f24f629?q=80&w=1640&auto=format&fit=crop",
    desc: "Miasteczko nad jeziorem, najlepsze o świcie poza główną promenadą.",
    tags: ["miasteczko", "jezioro", "spokój"],
    co2SavedKg: 4.2,
    calmBoost: 29,
  },
  {
    id: "4",
    name: "Cinque Terre",
    city: "Liguria",
    country: "Włochy",
    countryFlag: "🇮🇹",
    color: "violet",
    image:
      "https://images.unsplash.com/photo-1491553895911-0055eca6402d?q=80&w=1640&auto=format&fit=crop",
    desc: "Tarasowe ścieżki między wioskami. Najciszej poza godzinami szczytu.",
    tags: ["morze", "szlaki", "widoki"],
    co2SavedKg: 2.8,
    calmBoost: 24,
  },
  {
    id: "5",
    name: "Isle of Skye",
    city: "Highlands",
    country: "Szkocja",
    countryFlag: "🏴",
    color: "amber",
    image:
      "https://images.unsplash.com/photo-1441974231531-c6227db76b6e?q=80&w=1640&auto=format&fit=crop",
    desc: "Mgły, wrzosowiska i kaskady. Ścieżki z dala od parkingów = więcej ciszy.",
    tags: ["wzgórza", "wodospad", "wild"],
    co2SavedKg: 5.1,
    calmBoost: 36,
  },
];

// Tailwind helpers for colored location frame
const colorMap = {
  emerald: {
    ring: "ring-emerald-400/70",
    text: "text-emerald-300",
    bg: "bg-emerald-400/10",
  },
  teal: {
    ring: "ring-teal-400/70",
    text: "text-teal-300",
    bg: "bg-teal-400/10",
  },
  sky: { ring: "ring-sky-400/70", text: "text-sky-300", bg: "bg-sky-400/10" },
  violet: {
    ring: "ring-violet-400/70",
    text: "text-violet-300",
    bg: "bg-violet-400/10",
  },
  amber: {
    ring: "ring-amber-400/70",
    text: "text-amber-300",
    bg: "bg-amber-400/10",
  },
};

// ---------------------------------------------
// Card (Shorts/Reel) with swipe gestures
// ---------------------------------------------
function PlaceCard({ place, onAction }) {
  const [dragDir, setDragDir] = useState(null);

  const onDrag = (_e, info) => {
    const { x, y } = info.offset;
    if (Math.abs(x) > Math.abs(y)) setDragDir(x > 0 ? "right" : "left");
    else if (Math.abs(y) > 20) setDragDir(y > 0 ? "down" : "up");
  };

  const onDragEnd = (_e, info) => {
    const { x, y } = info.offset;
    const threshold = 160; // px
    if (x > threshold) onAction("like");
    else if (x < -threshold) onAction("nope");
    else if (y > threshold) onAction("save");
    setDragDir(null);
  };

  const c = colorMap[place.color] ?? colorMap.emerald;

  return (
    <motion.div
      key={place.id}
      className="relative w-[min(92vw,420px)] aspect-[9/16] rounded-3xl overflow-hidden shadow-2xl ring-1 ring-white/5 bg-zinc-900"
      initial={{ y: 24, opacity: 0 }}
      animate={{ y: 0, opacity: 1 }}
      exit={{ scale: 0.9, opacity: 0 }}
      transition={{ type: "spring", stiffness: 140, damping: 16 }}
      drag
      dragElastic={0.12}
      dragConstraints={{ left: 0, right: 0, top: 0, bottom: 0 }}
      onDrag={onDrag}
      onDragEnd={onDragEnd}
    >
      {/* Background image */}
      <img
        src={place.image}
        alt={place.name}
        className="absolute inset-0 w-full h-full object-cover"
      />
      {/* Gradient overlay */}
      <div className="absolute inset-0 bg-gradient-to-b from-black/40 via-black/30 to-black/70" />

      {/* Corner location frame */}
      <div
        className={`absolute top-3 left-3 rounded-xl px-3 py-2 backdrop-blur-sm ${
          c.bg
        } ring-2 ${c.ring} ${c.text} text-[13px] font-medium flex items-center gap-2`}
      >
        <Globe2 className="w-4 h-4" />
        <span>
          {place.city} • {place.countryFlag} {place.country}
        </span>
      </div>

      {/* Drag feedback badges */}
      <div className="absolute inset-0 pointer-events-none">
        <div
          className={`absolute top-8 left-6 rotate-[-12deg] border-2 rounded-lg px-2 py-1 text-sm tracking-widest ${
            dragDir === "right"
              ? "opacity-100 border-emerald-400 text-emerald-300"
              : "opacity-0"
          }`}
        >
          LIKE
        </div>
        <div
          className={`absolute top-8 right-6 rotate-[12deg] border-2 rounded-lg px-2 py-1 text-sm tracking-widest ${
            dragDir === "left"
              ? "opacity-100 border-rose-400 text-rose-300"
              : "opacity-0"
          }`}
        >
          NOPE
        </div>
        <div
          className={`absolute bottom-24 left-1/2 -translate-x-1/2 border-2 rounded-lg px-2 py-1 text-sm ${
            dragDir === "down"
              ? "opacity-100 border-amber-400 text-amber-300"
              : "opacity-0"
          }`}
        >
          SAVE
        </div>
      </div>

      {/* Bottom content */}
      <div className="absolute bottom-0 p-4 w-full">
        <div className="flex items-center gap-2 text-zinc-300 text-sm">
          <MapPin className="w-4 h-4" />
          <span className="truncate">
            {place.name} · {place.city}
          </span>
        </div>
        <p className="mt-2 text-zinc-200 text-[15px] leading-snug line-clamp-3">
          {place.desc}
        </p>
        <div className="mt-3 flex flex-wrap gap-2">
          {place.tags.map((t) => (
            <span
              key={t}
              className="px-2 py-1 rounded-full text-[11px] bg-white/5 text-zinc-300 ring-1 ring-white/10"
            >
              #{t}
            </span>
          ))}
        </div>

        {/* bottom controls */}
        <div className="mt-4 flex items-center justify-center gap-4">
          <button
            onClick={() => onAction("nope")}
            className="w-12 h-12 rounded-full grid place-items-center bg-white/5 ring-1 ring-white/10 hover:bg-rose-500/20 hover:ring-rose-500/40 transition"
            title="Odrzuć (←)"
          >
            <Close className="w-6 h-6" />
          </button>
          <button
            onClick={() => onAction("save")}
            className="w-12 h-12 rounded-full grid place-items-center bg-white/5 ring-1 ring-white/10 hover:bg-amber-500/20 hover:ring-amber-500/40 transition"
            title="Zapisz (↓)"
          >
            <Bookmark className="w-6 h-6" />
          </button>
          <button
            onClick={() => onAction("like")}
            className="w-12 h-12 rounded-full grid place-items-center bg-white/5 ring-1 ring-white/10 hover:bg-emerald-500/20 hover:ring-emerald-500/40 transition"
            title="Lubię (→)"
          >
            <Heart className="w-6 h-6" />
          </button>
        </div>
      </div>
    </motion.div>
  );
}

// ---------------------------------------------
// Stat widget
// ---------------------------------------------
function Stat({ icon: Icon, label, value, sub }) {
  return (
    <div className="p-4 rounded-2xl bg-white/5 ring-1 ring-white/10">
      <div className="flex items-center gap-2 text-zinc-300">
        <Icon className="w-4 h-4" />
        <span className="text-sm">{label}</span>
      </div>
      <div className="mt-2 text-2xl font-semibold text-white">{value}</div>
      {sub && <div className="text-xs mt-1 text-zinc-400">{sub}</div>}
    </div>
  );
}

// ---------------------------------------------
// Main Dashboard
// ---------------------------------------------
export default function CalmPlansDashboard() {
  const [index, setIndex] = useState(0);
  const [liked, setLiked] = useState([]); // ids
  const [saved, setSaved] = useState([]);
  const [nope, setNope] = useState([]);

  const active = PLACES[index];

  const handleAction = useCallback(
    (type) => {
      if (!active) return;
      if (type === "like") setLiked((p) => [...p, active.id]);
      if (type === "save") setSaved((p) => [...p, active.id]);
      if (type === "nope") setNope((p) => [...p, active.id]);
      setIndex((i) => Math.min(i + 1, PLACES.length));
    },
    [active]
  );

  // Keyboard shortcuts
  useEffect(() => {
    const onKey = (e) => {
      if (!active) return;
      if (e.key === "ArrowRight") handleAction("like");
      if (e.key === "ArrowLeft") handleAction("nope");
      if (e.key === "ArrowDown") handleAction("save");
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [active, handleAction]);

  // Stats
  const carbonSaved = useMemo(() => {
    const ids = new Set([...liked, ...saved]);
    return PLACES.filter((p) => ids.has(p.id)).reduce((acc, p) => acc + p.co2SavedKg, 0);
  }, [liked, saved]);

  const calmBoost = useMemo(() => {
    const ids = new Set([...liked, ...saved]);
    if (ids.size === 0) return 0;
    const sum = PLACES.filter((p) => ids.has(p.id)).reduce((acc, p) => acc + p.calmBoost, 0);
    return Math.round(sum / ids.size);
  }, [liked, saved]);

  const savedPlaces = PLACES.filter((p) => saved.includes(p.id));

  return (
    <div className="min-h-screen bg-gradient-to-b from-zinc-950 via-zinc-950 to-black text-white">
      {/* Top bar */}
      <header className="sticky top-0 z-20 border-b border-white/5 backdrop-blur bg-black/30">
        <div className="mx-auto max-w-7xl px-4 h-14 flex items-center justify-between">
          <div className="flex items-center gap-2 select-none">
            <div className="w-8 h-8 rounded-xl bg-emerald-500/90 shadow-[0_0_20px_rgba(16,185,129,0.6)] grid place-items-center">
              <svg viewBox="0 0 24 24" className="w-5 h-5" fill="none" stroke="white" strokeWidth="1.8">
                <path d="M3 12c4-6 14-6 18 0" /><path d="M7 12c2.5-3.5 7.5-3.5 10 0" /><circle cx="12" cy="12" r="1.2" fill="white"/>
              </svg>
            </div>
            <span className="text-lg font-semibold tracking-tight">CalmPlans</span>
          </div>
          <nav className="hidden md:flex items-center gap-6 text-sm text-zinc-300">
            <a href="#reel" className="hover:text-white">Reel</a>
            <a href="#stats" className="hover:text-white">Statystyki</a>
            <a href="#saved" className="hover:text-white">Zapisane</a>
            <a href="#settings" className="hover:text-white flex items-center gap-2"><Settings className="w-4 h-4"/>Ustawienia</a>
          </nav>
          <div className="flex items-center gap-3">
            <button className="px-3 py-1.5 rounded-xl text-xs bg-white/5 ring-1 ring-white/10">Demo</button>
            <button className="px-3 py-1.5 rounded-xl text-xs bg-emerald-500/90 hover:bg-emerald-400 text-zinc-900 font-medium shadow-[0_0_20px_rgba(16,185,129,0.5)]">Zacznij</button>
          </div>
        </div>
      </header>

      {/* Content */}
      <main className="mx-auto max-w-7xl px-4 py-8 grid md:grid-cols-[1fr_360px] gap-8">
        {/* Reel / Tinder-like */}
        <section id="reel" className="flex flex-col items-center">
          <div className="text-3xl md:text-4xl font-bold leading-tight">
            Spokojne odkrywanie miejsc
          </div>
          <p className="mt-2 text-zinc-400 text-sm">Przeciągnij w prawo aby polubić, w lewo aby pominąć, w dół aby zapisać. 
            <span className="ml-2">Skróty: ←/→/↓</span>
          </p>

          <div className="mt-6 relative w-full flex items-center justify-center">
            <AnimatePresence initial={false} mode="popLayout">
              {active ? (
                <PlaceCard key={active.id} place={active} onAction={handleAction} />
              ) : (
                <motion.div
                  initial={{ opacity: 0 }}
                  animate={{ opacity: 1 }}
                  className="w-[min(92vw,420px)] aspect-[9/16] rounded-3xl grid place-items-center bg-white/5 ring-1 ring-white/10 text-center p-6"
                >
                  <Sparkles className="w-7 h-7 text-emerald-300 mb-2" />
                  <div className="text-lg font-medium">Koniec propozycji na dziś 🎉</div>
                  <div className="text-sm text-zinc-400 mt-1">Zmień filtry lub odśwież, aby zobaczyć nowe miejsca.</div>
                </motion.div>
              )}
            </AnimatePresence>
          </div>
        </section>

        {/* Right sidebar */}
        <aside className="space-y-4" id="stats">
          <Stat
            icon={Leaf}
            label="Mniejszy ślad węglowy"
            value={`${carbonSaved.toFixed(1)} kg CO₂`}
            sub="Szacunek dla polubionych i zapisanych miejsc"
          />
          <Stat
            icon={Wind}
            label="Spokój wypoczynku"
            value={`${calmBoost}%`}
            sub="Średni wzrost spokoju względem miejsc turystycznych"
          />

          {/* Saved */}
          <div id="saved" className="p-4 rounded-2xl bg-white/5 ring-1 ring-white/10">
            <div className="flex items-center gap-2 text-zinc-300">
              <Bookmark className="w-4 h-4" />
              <span className="text-sm">Zapisane miejsca</span>
            </div>
            <div className="mt-3 space-y-3">
              {savedPlaces.length === 0 && (
                <div className="text-sm text-zinc-400">Na razie pusto. Przeciągnij kartę w dół, aby zapisać.</div>
              )}
              {savedPlaces.map((p) => (
                <div key={p.id} className="flex items-center gap-3">
                  <img src={p.image} alt={p.name} className="w-12 h-12 object-cover rounded-lg" />
                  <div className="min-w-0">
                    <div className="text-sm text-white truncate">{p.name}</div>
                    <div className="text-xs text-zinc-400 truncate">{p.city} • {p.country}</div>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {/* Settings */}
          <div id="settings" className="p-4 rounded-2xl bg-white/5 ring-1 ring-white/10">
            <div className="flex items-center gap-2 text-zinc-300">
              <Settings className="w-4 h-4" />
              <span className="text-sm">Ustawienia podpowiedzi</span>
            </div>
            <div className="mt-3 space-y-2 text-sm text-zinc-300">
              <label className="flex items-center gap-2">
                <input type="checkbox" defaultChecked className="accent-emerald-500" />
                Preferuj miejsca z niskim ruchem
              </label>
              <label className="flex items-center gap-2">
                <input type="checkbox" defaultChecked className="accent-emerald-500" />
                Priorytet ścieżek wśród zieleni
              </label>
              <label className="flex items-center gap-2">
                <input type="checkbox" className="accent-emerald-500" />
                Dostępność offline
              </label>
            </div>
          </div>
        </aside>
      </main>

      {/* Footer hint */}
      <footer className="px-4 py-8 text-center text-zinc-500 text-xs">
        Swipe UI • CalmPlans © 2025
      </footer>
    </div>
  );
}
