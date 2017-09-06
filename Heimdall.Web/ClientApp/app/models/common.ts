export interface IApiErrorResult {
    message: string;
    details: IApiError[];
}

export interface IApiError {
    context: string;
    message: string;
}