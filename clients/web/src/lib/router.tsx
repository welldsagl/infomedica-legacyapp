import { createRootRoute, createRoute, createRouter, redirect } from '@tanstack/react-router'
import { validateClientiSearch } from '@/features/clienti/utils/mappers'
import RootLayout from '@/App'
import { ClienteCreatePage, ClienteDetailPage, ClienteEditPage, ClientiListPage } from './pages'

const rootRoute = createRootRoute({ component: RootLayout })

const indexRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/',
  beforeLoad: () => {
    // eslint-disable-next-line @typescript-eslint/only-throw-error -- API di TanStack Router
    throw redirect({ to: '/clienti', search: { page: 1 } })
  },
})

const clientiRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/clienti',
  validateSearch: validateClientiSearch,
  component: ClientiListPage,
})

const clienteCreateRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/clienti/nuovo',
  component: ClienteCreatePage,
})

const clienteDetailRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/clienti/$id',
  component: ClienteDetailPage,
})

const clienteEditRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: '/clienti/$id/modifica',
  component: ClienteEditPage,
})

const routeTree = rootRoute.addChildren([
  indexRoute,
  clientiRoute,
  clienteCreateRoute,
  clienteDetailRoute,
  clienteEditRoute,
])

export const router = createRouter({ routeTree, defaultPreload: 'intent' })

// Registrazione dei tipi: rende tipizzati to/params/search in tutta l'app.
// Module augmentation di TanStack Router: richiede "interface".
declare module '@tanstack/react-router' {
  // eslint-disable-next-line @typescript-eslint/consistent-type-definitions -- module augmentation
  interface Register {
    router: typeof router
  }
}
