import Link from "next/link";
import { useState } from "react";

export const NinjaSideBar = () => {
    const [isOpen, setIsOpen] = useState(false);

    const handleToggle = () => {
        setIsOpen(!isOpen);
    };

    return (
        <div
            className={`${isOpen ? "w-64" : "w-0"
                } md:w-64 bg-gray-900 p-6 transition-width duration-300`}
        >
            <button
                className="md:hidden text-white text-2xl"
                onClick={handleToggle}
            >
                &#9776;
            </button>
            {isOpen && (
                <div className="mt-4 flex flex-col h-full justify-between ">
                    <div className="accordion">
                        <div className="accordion-item">
                            <div className="accordion-header">
                                <h2 className="text-white font-bold">Basic</h2>
                            </div>
                            <div className="accordion-body">
                                <ul>
                                    <li>
                                        <Link href="/customers" className="text-white">
                                            Customers
                                        </Link>
                                    </li>
                                    <li>
                                        <Link href="/suppliers" className="text-white">
                                            Suppliers
                                        </Link>
                                    </li>
                                </ul>
                            </div>
                        </div>
                        <div className="accordion-item">
                            <div className="accordion-header">
                                <h2 className="text-white font-bold">Projects</h2>
                            </div>
                            <div className="accordion-body">
                                <ul>
                                    <li>
                                        <Link href="/projects" className="text-white">
                                            Projects
                                        </Link>
                                    </li>
                                    <li>
                                        <Link href="/tasks" className="text-white">
                                            Tasks
                                        </Link>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                </div>
            )
            }
        </div>
    );
};
