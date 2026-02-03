export interface IPagination<T> {
  data: T;
  total: number;
  page: number;
  perPage: number;
  pages: number;
}
