import { useEditClienteForm } from '../hooks/useEditClienteForm'
import ClienteForm from './ClienteForm'
import type { Cliente } from '../types'

type ClienteEditFormProps = {
  cliente: Cliente
}

// Montato solo quando il cliente e' caricato: i defaultValues del form sono gia' quelli giusti.
const ClienteEditForm = ({ cliente }: ClienteEditFormProps) => {
  const { form, onSubmit, cancel, isPending } = useEditClienteForm(cliente)

  return (
    <ClienteForm
      form={form}
      onSubmit={onSubmit}
      onCancel={cancel}
      isPending={isPending}
      submitLabel="Salva modifiche"
    />
  )
}

export default ClienteEditForm
