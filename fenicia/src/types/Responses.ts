export type TokenResponse = {
  accessToken: string;
  refreshToken: string;
  user: UserResponse;
}

export type UserResponse = {
  id: string;
  name: string;
  email: string;
}

export type CompanyResponse = {
  id: string;
  name: string;
  cnpj: string;
  language: string;
  timezone: string;
}

export type ModuleResponse = {
  id: string;
  name: string;
  amout: number;
  type: EnumModuleType;
  submodules: SubmoduleResponse[];
}

export type SubmoduleResponse = {
  id: string;
  name: string;
  description?: string;
  route: string;

}

export enum EnumModuleType {
  Erp = -1,
  Auth = 0,
  Basic = 1,
  SocialNetwork = 2,
  Project = 3,
  PerformanceEvaluation = 4,
  Accounting = 5,
  Hr = 6,
  Pos = 7,
  Contracts = 8,
  Ecommerce = 9,
  CustomerSupport = 10,
  Plus = 11
}
