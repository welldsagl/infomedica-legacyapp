import { Badge, Tooltip } from '@mantine/core'
import { useApiHealth } from '@/hooks/useApiHealth'

const ApiStatusBadge = () => {
  const { data, isPending, isError } = useApiHealth()

  if (isPending)
    return (
      <Badge color="gray" variant="light">
        API…
      </Badge>
    )
  if (isError)
    return (
      <Badge color="red" variant="light">
        API non raggiungibile
      </Badge>
    )

  return (
    <Tooltip label={`Processo ${data.is64BitProcess ? '64' : '32'} bit`}>
      <Badge color={data.status === 'ok' ? 'green' : 'orange'} variant="light">
        {data.provider} · {data.status}
      </Badge>
    </Tooltip>
  )
}

export default ApiStatusBadge
