export interface IBaseResponseWithPaging<TEntity> {

    page: number;
    totalPageCount: number;
    data: TEntity[];
}
