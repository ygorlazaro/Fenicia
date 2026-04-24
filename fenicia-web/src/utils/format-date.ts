export default (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
};
