import { buscaCEP } from 'buscadorcep';

export interface CepAddress {
  cep: string;
  state: string;
  city: string;
  neighborhood: string;
  street: string;
  complement?: string;
}

export const fetchAddressByCep = async (cep: string): Promise<CepAddress | null> => {
  try {
    const result = await buscaCEP(cep);
    if (result) {
      return {
        cep: result.cep || cep,
        state: result.uf || '',
        city: result.localidade || '',
        neighborhood: result.bairro || '',
        street: result.logradouro || '',
        complement: result.complemento || ''
      };
    }
    return null;
  } catch (error) {
    console.error('Failed to fetch address by CEP:', error);
    return null;
  }
};

export default { fetchAddressByCep };
