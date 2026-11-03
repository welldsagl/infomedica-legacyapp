import { Stack, Title } from '@mantine/core'
import { useCreateClienteForm } from '../hooks/useCreateClienteForm'
import ClienteForm from './ClienteForm'

const ClienteCreateContent = () => {
  const { form, onSubmit, cancel, isPending } = useCreateClienteForm()

  return (
    <Stack gap="md">
      <Title order={2}>Nuovo cliente</Title>
      <ClienteForm
        form={form}
        onSubmit={onSubmit}
        onCancel={cancel}
        isPending={isPending}
        submitLabel="Crea cliente"
      />
    </Stack>
  )
}

export default ClienteCreateContent
