import { Suspense } from 'react'
import { AppShell, Container, Group, Title } from '@mantine/core'
import { Outlet } from '@tanstack/react-router'
import { AnchorLink, ApiStatusBadge, PageLoader } from '@/components'
import { config } from '@/config'

const RootLayout = () => (
  <AppShell header={{ height: 60 }} padding="md">
    <AppShell.Header>
      <Container size="xl" className="flex h-full items-center">
        <Group justify="space-between" className="w-full">
          <Group gap="lg">
            <Title order={4}>{config.appName}</Title>
            <AnchorLink to="/clienti" search={{ page: 1 }} c="dimmed">
              Clienti
            </AnchorLink>
          </Group>
          <ApiStatusBadge />
        </Group>
      </Container>
    </AppShell.Header>

    <AppShell.Main>
      <Container size="xl">
        <Suspense fallback={<PageLoader />}>
          <Outlet />
        </Suspense>
      </Container>
    </AppShell.Main>
  </AppShell>
)

export default RootLayout
