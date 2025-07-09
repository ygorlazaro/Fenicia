interface NinjaProps {
    label: string;
    onClick: () => void;
    disabled?: boolean;
}

export const NinjaButton = ({label, onClick, disabled}: NinjaProps) => {
    return <button onClick={onClick} disabled={disabled} className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">{label}</button>
}
