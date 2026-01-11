import { useState } from "react";
import { X } from "lucide-react";

interface SkillBadgeProps {
  skill: string;
  onRemove: (skill: string) => void;
  index: number;
}

const SkillBadge = ({ skill, onRemove, index }: SkillBadgeProps) => {
  const [isRemoving, setIsRemoving] = useState(false);

  const handleRemove = () => {
    setIsRemoving(true);
    setTimeout(() => onRemove(skill), 200); // remove after animation
  };

  return (
    <div
      className={`skill-badge ${isRemoving ? "removing" : ""}`}
      style={{ animationDelay: `${index * 50}ms` }}
    >
      <span>{skill}</span>
      <button onClick={handleRemove} aria-label="حذف المهارة">
        <X width={12} height={12} />
      </button>
    </div>
  );
};

export default SkillBadge;
