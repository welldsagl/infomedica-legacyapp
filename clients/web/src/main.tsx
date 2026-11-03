import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { MantineProvider, createTheme } from '@mantine/core'
import { ModalsProvider } from '@mantine/modals'
import { Notifications } from '@mantine/notifications'
import { QueryClientProvider } from '@tanstack/react-query'
import { RouterProvider } from '@tanstack/react-router'
import { ErrorBoundary } from 'react-error-boundary'
import { ErrorFallback } from '@/components'
import { queryClient } from '@/lib/queryClient'
import { router } from '@/lib/router'
import '@mantine/core/styles.css'
import '@mantine/notifications/styles.css'
import './index.css'

const theme = createTheme({
  primaryColor: 'blue',
  defaultRadius: 'md',
})

const rootElement = document.getElementById('root')
if (rootElement === null) throw new Error('Elemento #root non trovato')

createRoot(rootElement).render(
  <StrictMode>
    <MantineProvider theme={theme} defaultColorScheme="auto">
      <ErrorBoundary FallbackComponent={ErrorFallback}>
        <QueryClientProvider client={queryClient}>
          <ModalsProvider>
            <Notifications position="top-right" />
            <RouterProvider router={router} />
          </ModalsProvider>
        </QueryClientProvider>
      </ErrorBoundary>
    </MantineProvider>
  </StrictMode>
)
