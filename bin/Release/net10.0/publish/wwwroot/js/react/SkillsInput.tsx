import { useState } from "react";
import SkillBadge from "./SkillBadge"; // adjust path if needed

const SkillsInput = () => {
  const [skills, setSkills] = useState<string[]>(["JavaScript", "React", "TypeScript"]);

  const handleRemove = (skillToRemove: string) => {
    setSkills(skills.filter(skill => skill !== skillToRemove));
  };

  return (
    <div className="skills-container">
      {skills.map((skill, index) => (
        <SkillBadge
          key={skill}
          skill={skill}
          onRemove={handleRemove}
          index={index}
        />
      ))}
    </div>
  );
};

export default SkillsInput;
