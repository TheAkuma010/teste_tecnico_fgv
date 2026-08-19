import type { Config } from "tailwindcss";

const config: Config = {
  content: [
    "./app/**/*.{js,ts,jsx,tsx,mdx}",
    "./components/**/*.{js,ts,jsx,tsx,mdx}"
  ],
  theme: {
    extend: {
      colors: {
        ink: "#172026",
        line: "#d8dee4",
        brand: "#0f766e",
        danger: "#b42318"
      }
    }
  },
  plugins: []
};

export default config;
