import { lazy } from 'react'

// Ogni pagina e' caricata on demand (code splitting); il <Suspense> e' in RootLayout.
export const ClientiListPage = lazy(() => import('@/features/clienti/ClientiListPage'))
export const ClienteCreatePage = lazy(() => import('@/features/clienti/ClienteCreatePage'))
export const ClienteDetailPage = lazy(() => import('@/features/clienti/ClienteDetailPage'))
export const ClienteEditPage = lazy(() => import('@/features/clienti/ClienteEditPage'))
