export type TokenRequest = {
  email: string;
  password: string
}

export type SignUpRequest = {
  email: string;
  password: string;
  name: string;
  company: CompanyRequest;
}

export type CompanyRequest = {
  name: string;
  cnpj: string;
}

export type ForgotPasswordRequest = {
  email: string;
}

export type RecoverPasswordRequest = {
  email: string;
  code: string;
  password: string;
}
