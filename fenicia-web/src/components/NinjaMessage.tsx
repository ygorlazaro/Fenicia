interface NinjaProps {
    message?: string
}

export const NinjaMessage = ({ message }: NinjaProps) => {
    
    if (!message) {
        return null;
    }
    
    return (<div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded relative mt-8">
        {message}
    </div>)
}
