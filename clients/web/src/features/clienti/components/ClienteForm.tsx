import { Alert, Button, Checkbox, Group, Stack, TextInput } from '@mantine/core'
import { Controller } from 'react-hook-form'
import type { SubmitEvent } from 'react'
import type { UseFormReturn } from 'react-hook-form'
import type { ClienteFormOutput, ClienteFormValues } from '../types'

type ClienteFormProps = {
  form: UseFormReturn<ClienteFormValues, unknown, ClienteFormOutput>
  onSubmit: (event: SubmitEvent<HTMLFormElement>) => void
  onCancel: () => void
  isPending: boolean
  submitLabel: string
}

// Pura resa grafica: stato e validazione arrivano dagli hook useCreateClienteForm / useEditClienteForm.
const ClienteForm = ({ form, onSubmit, onCancel, isPending, submitLabel }: ClienteFormProps) => {
  const {
    control,
    formState: { errors },
  } = form

  return (
    <form onSubmit={onSubmit} noValidate>
      <Stack gap="md" className="max-w-xl">
        {errors.root?.server?.message !== undefined && (
          <Alert color="red">{errors.root.server.message}</Alert>
        )}

        <Controller
          name="ragioneSociale"
          control={control}
          render={({ field }) => (
            <TextInput
              {...field}
              label="Ragione sociale"
              withAsterisk
              error={errors.ragioneSociale?.message}
            />
          )}
        />
        <Controller
          name="partitaIva"
          control={control}
          render={({ field }) => (
            <TextInput
              {...field}
              label="Partita IVA"
              maxLength={11}
              error={errors.partitaIva?.message}
            />
          )}
        />
        <Controller
          name="email"
          control={control}
          render={({ field }) => (
            <TextInput {...field} type="email" label="Email" error={errors.email?.message} />
          )}
        />
        <Controller
          name="attivo"
          control={control}
          render={({ field: { value, onChange, onBlur, name, ref } }) => (
            <Checkbox
              ref={ref}
              name={name}
              label="Cliente attivo"
              checked={value}
              onBlur={onBlur}
              onChange={(e) => {
                onChange(e.currentTarget.checked)
              }}
            />
          )}
        />

        <Group justify="flex-end">
          <Button variant="default" onClick={onCancel} disabled={isPending}>
            Annulla
          </Button>
          <Button type="submit" loading={isPending}>
            {submitLabel}
          </Button>
        </Group>
      </Stack>
    </form>
  )
}

export default ClienteForm
