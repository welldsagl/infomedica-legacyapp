import { Schema } from 'effect'

/** Specchio di LegacyApp.Api.Contracts.PagedResult(Of T). */
export const PagedResultSchema = <A, I>(item: Schema.Schema<A, I>) =>
  Schema.Struct({
    items: Schema.Array(item),
    page: Schema.Number,
    pageSize: Schema.Number,
    totalCount: Schema.Number,
  })

export type PagedResult<A> = {
  readonly items: ReadonlyArray<A>
  readonly page: number
  readonly pageSize: number
  readonly totalCount: number
}
