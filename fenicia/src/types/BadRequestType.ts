export interface BadRequestType {
  type: string;
  title: string;
  status: number;
  traceId: string;
  detail: string;
  errors: ValidationErrorMap;
}

export interface ValidationErrorMap {
  [key: string]: string[] | ValidationErrorMap | any;
}
