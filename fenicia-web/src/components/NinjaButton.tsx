import { NinjaSnipper } from "./NinjaSnipper";

interface NinjaProps {
    label: string;
    onClick: () => void;
    disabled?: boolean;
    loading?: boolean;
    className?: string;
}

export const NinjaButton = ({ label, onClick, disabled, loading, className }: NinjaProps) => {
    return (<button onClick={onClick}
        disabled={disabled || loading}
        className={(className ?? "") + "text-orange-700 hover:text-white border border-orange-700 hover:bg-orange-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5 text-center  mb-2"}>
        {loading ? <NinjaSnipper /> : label}
    </button>)
}
