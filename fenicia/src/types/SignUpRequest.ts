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
