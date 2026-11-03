import type { Config } from 'tailwindcss'

export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  // Preflight disattivato: il reset CSS lo fa gia' Mantine
  corePlugins: { preflight: false },
} satisfies Config
