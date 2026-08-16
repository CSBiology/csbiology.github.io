export const rotationClasses = {
  none: "",
  left: "-rotate-[1.2deg]",
  right: "rotate-[1.4deg]",
} as const;

export type Rotation = keyof typeof rotationClasses;
