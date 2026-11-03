const dateFormatter = new Intl.DateTimeFormat('it-IT', { dateStyle: 'short' })
const dateTimeFormatter = new Intl.DateTimeFormat('it-IT', {
  dateStyle: 'short',
  timeStyle: 'short',
})
// useGrouping 'always': in italiano CLDR omette il separatore sotto 10.000 (1350,40 -> 1.350,40)
const currencyFormatter = new Intl.NumberFormat('it-IT', {
  style: 'currency',
  currency: 'EUR',
  useGrouping: 'always',
})

export const formatDate = (date: Date): string => dateFormatter.format(date)

export const formatDateTime = (date: Date): string => dateTimeFormatter.format(date)

export const formatCurrency = (value: number): string => currencyFormatter.format(value)

export const orDash = (value: string | null): string => value ?? '—'
