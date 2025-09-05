import "./DemoCard.css";
import { motion, useMotionValue, animate, useTransform } from "framer-motion";
import { MapPin, Earth, XIcon, SaveIcon, Heart } from "lucide-react";
import tatryImage from "../../../assets/pictures/tatry.jpeg";

const DemoCard = ({ location, country, title, description, tags = [], image }) => {
  const x = useMotionValue(0);
  const y = useMotionValue(0);

  const likeOpacity = useTransform(x, [0, 180, 220], [0, 0, 1]);
  const dislikeOpacity = useTransform(x, [0, -180, -220], [0, 0, 1]);
  const likeScale = useTransform(x, [0, 180, 260], [0.8, 1, 1.2]);
  const dislikeScale = useTransform(x, [0, -180, -260], [0.8, 1, 1.2]);
  const rotate = useTransform(x, [-100, 0, 100], [-5, 0, 5]);

  return (
    <div className="demo-card-wrapper">
      <motion.div
        className="demo-card"
        drag
        dragElastic={0.2}
        dragMomentum={false}
        dragConstraints={{ left: -100, right: 100, top: -50, bottom: 50 }}
        style={{ 
          cursor: "grab", 
          x, 
          y,
          rotate
        }}
        whileTap={{ cursor: "grabbing" }}
        onDragEnd={() => {
          animate(x, 0, { type: "spring", stiffness: 300, damping: 20 });
          animate(y, 0, { type: "spring", stiffness: 300, damping: 20 });
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
        <div className="card-image-container">
          <img src={tatryImage} alt={title} className="card-image" />
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
            <button className="action-btn"><XIcon strokeWidth={2} /></button>
            <button className="action-btn"><SaveIcon strokeWidth={2} /></button>
            <button className="action-btn"><Heart strokeWidth={2} /></button>
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default DemoCard;