
export const formatNumber = (value: number) => {
    return new Intl.NumberFormat('pt-BR').format(value)
}
