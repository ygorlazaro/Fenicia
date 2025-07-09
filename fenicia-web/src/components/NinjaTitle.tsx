interface NinjaProps {
    label: string
}

export const NinjaTitle = ({ label }: NinjaProps) => {
    return (<h1 className="mb-8 text-4xl font-extrabold text-gray-900 dark:text-white">
        <span className="text-transparent bg-clip-text bg-gradient-to-r to-orange-300 from-orange-600">{label}
        </span>
    </h1>)
}
