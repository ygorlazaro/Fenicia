export type LoginRequest = {
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
