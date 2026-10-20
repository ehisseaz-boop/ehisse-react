import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// Lokal backend işləmirsə, avtomatik canlı onrender backend-ə qoşulur
const backendTarget = process.env.VITE_BACKEND_TARGET || 'https://ehisse-backend.onrender.com'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5175,
    strictPort: true,
    // Bütün /api və /Image sorğuları arxa planda backend-ə ötürülür.
    // Brauzerin URL-i həmişə localhost:5175 qalır, cookie-lər də eyni origin-də işləyir.
    proxy: {
      '/api': { target: backendTarget, changeOrigin: true, secure: false },
      '/Image': { target: backendTarget, changeOrigin: true, secure: false },
      '/images': { target: backendTarget, changeOrigin: true, secure: false },
      '/uploads': { target: backendTarget, changeOrigin: true, secure: false },
    },
  },
})
