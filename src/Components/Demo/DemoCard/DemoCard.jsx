import "./DemoCard.css";
import { motion, useMotionValue, animate, useTransform, useMotionValueEvent } from "framer-motion";
import { MapPin, Earth, XIcon, SaveIcon, Heart } from "lucide-react";
import { useState } from "react";

const DemoCard = ({ location, country, title, description, tags = [], image, onSwipeUp, onSwipeDown }) => {
  const x = useMotionValue(0);
  const y = useMotionValue(0);

  const likeOpacity = useTransform(x, [0, 60, 100], [0, 0, 1]);
  const dislikeOpacity = useTransform(x, [0, -60, -100], [0, 0, 1]);
  const saveOpacity = useTransform(y, [0, 60, 100], [0, 0, 1]);
  const skipOpacity = useTransform(y, [0, -60, -100], [0, 0, 1]);
  const [showSaveIcon, setShowSaveIcon] = useState(true);

  useMotionValueEvent(y, "change", (latest) => {
    setShowSaveIcon(latest < 60);
  });

  const likeScale = useTransform(x, [0, 60, 100], [0.8, 1, 1.2]);
  const dislikeScale = useTransform(x, [0, -60, -100], [0.8, 1, 1.2]);
  const saveScale = useTransform(y, [0, 60, 100], [0.8, 1, 1.3]);
  const skipScale = useTransform(y, [0, -60, -100], [0.8, 1, 1.2]);

  const rotate = useTransform(x, [-100, 0, 100], [-5, 0, 5]);

  return (
    <div className="demo-card-wrapper">
      <motion.div
        className="demo-card"
        drag
        dragElastic={0.2}
        dragMomentum={false}
        dragConstraints={{ left: -30, right: 30, top: -50, bottom: 50 }}

        style={{ 
          cursor: "grab", 
          x, 
          y,
          rotate
        }}
        whileTap={{ cursor: "grabbing" }}
        onDragEnd={(event, info) => {
          const offsetY = info.offset.y;
          const offsetX = info.offset.x;

          if (offsetY < -180 || offsetY > 190 || offsetX > 200 || offsetX < -200) {
            onSwipeUp();
            animate(x, 0, { type: "spring", stiffness: 300, damping: 20 });
            animate(y, 0, { type: "spring", stiffness: 300, damping: 20 });
          }

          if (offsetY > 190) {
            onSwipeDown();
            animate(x, 0, { type: "spring", stiffness: 300, damping: 20 });
            animate(y, 0, { type: "spring", stiffness: 300, damping: 20 });
          }

          if (offsetY > 180) {
            animate(x, 0, { type: "spring", stiffness: 300, damping: 20 });
            animate(y, 0, { type: "spring", stiffness: 300, damping: 20 });
          }
        }}

      >
        <motion.div
          className="card-label card-label-dislike"
          style={{
            opacity: dislikeOpacity,
            scale: dislikeScale,
          }}
        >
          DISLIKE
        </motion.div>
        <motion.div
          className="card-label card-label-like"
          style={{
            opacity: likeOpacity,
            scale: likeScale,
          }}
        >
          LIKE
        </motion.div>
        <motion.div
          className="card-label card-label-save"
          style={{
            opacity: saveOpacity,
            scale: saveScale
          }}
        >
          SAVE
        </motion.div>
        <motion.div
          className="card-label card-label-skip"
          style={{
            opacity: skipOpacity,
            scale: skipScale
          }}
        >
          SKIP
        </motion.div>
        <div className="card-image-container">
          <img src={image} alt={title} className="card-image" />
          <div className="location-tag">
            <span className="earth-icon"><Earth /></span>
            <span>{location} • {country}</span>
          </div>
        </div>
        <div className="card-content">
          <div className="card-location">
            <span className="location-pin"><MapPin /></span>
            <span>{title}</span>
          </div>
          <p className="card-description">{description}</p>
          <div className="card-tags">
            {tags.map((tag, index) => (
              <span key={index} className="tag">#{tag}</span>
            ))}
          </div>
          <div className="card-actions">
            <button className={`action-btn action-btn-x${!showSaveIcon ? " move-left" : ""}`}>
              <XIcon strokeWidth={2} />
            </button>
            {showSaveIcon && (
              <button className="action-btn">
                <SaveIcon strokeWidth={2} />
              </button>
            )}
            <button className={`action-btn action-btn-heart${!showSaveIcon ? " move-right" : ""}`}>
              <Heart strokeWidth={2} />
            </button>
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default DemoCard;