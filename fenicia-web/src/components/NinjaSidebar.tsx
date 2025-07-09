"use client";

import { modules } from "@/helpers/modules";
import { getUser, removeToken, removeUser } from "@/logic/token";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { NinjaButton } from "./NinjaButton";
import { NinjaLink } from "./NinjaLink";

export const NinjaSideBar = () => {
    const [selectedModule, setSelectedModule] = useState<string | null>(null);
    const [modulesMenu, setModulesMenu] = useState(false);
    const [profileMenu, setProfileMenu] = useState(false);

    const user = getUser();
    const router = useRouter();

    const signOut = () => {
        removeUser();
        removeToken();
        router.push("/");
    };

    return (
        <nav className="bg-gray-800 border-gray-700">
            <div className="max-w-screen-xl flex items-center justify-between mx-auto p-4 relative">
                <a href="#" className="flex items-center space-x-3 text-white">
                    <img src="logo.jpeg" className="h-8" alt="Fenicia" />
                    <span className="text-2xl font-semibold whitespace-nowrap">Fenicia</span>
                </a>

                <div id="navbar" className="relative">
                    <ul className="flex flex-row space-x-8 font-medium">
                        <li>
                            <a
                                href="#"
                                className="flex items-center py-2 px-3 text-white hover:bg-gray-400 rounded"
                            >
                                Home
                            </a>
                        </li>
                        <li>
                            <div className="relative">
                                <button
                                    onClick={() => setModulesMenu(!modulesMenu)}
                                    className="flex items-center py-2 px-3 text-white hover:bg-gray-400 rounded"
                                >
                                    Modules
                                    <svg className="w-3 h-3 ml-2" fill="none" viewBox="0 0 10 6">
                                        <path
                                            stroke="currentColor"
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                            strokeWidth="2"
                                            d="m1 1 4 4 4-4"
                                        />
                                    </svg>
                                </button>

                                {modulesMenu && (
                                    <div className="absolute left-0 mt-2 bg-white border border-gray-200 rounded shadow-md w-56 z-50">
                                        <ul className="py-2 text-sm text-gray-700 max-h-80 overflow-y-auto">
                                            {modules.map((module, idx) => (
                                                <li key={idx}>
                                                    <button
                                                        onClick={() =>
                                                            selectedModule === module.name
                                                                ? setSelectedModule(null)
                                                                : setSelectedModule(module.name)
                                                        }
                                                        className="flex justify-between w-full px-4 py-2 hover:bg-gray-100"
                                                    >
                                                        {module.name}
                                                        <svg className="w-3 h-3 ml-2" fill="none" viewBox="0 0 10 6">
                                                            <path
                                                                stroke="currentColor"
                                                                strokeLinecap="round"
                                                                strokeLinejoin="round"
                                                                strokeWidth="2"
                                                                d="m1 1 4 4 4-4"
                                                            />
                                                        </svg>
                                                    </button>
                                                    {selectedModule === module.name && (
                                                        <div className="pl-4">
                                                            <ul className="py-1">
                                                                {module.routes.map((route, i) => (
                                                                    <li key={i}>
                                                                        <NinjaLink
                                                                            label={route.name}
                                                                            href={route.href}
                                                                            className="block px-4 py-2 hover:bg-gray-100 text-left w-full"
                                                                        />
                                                                    </li>
                                                                ))}
                                                            </ul>
                                                        </div>
                                                    )}
                                                </li>
                                            ))}
                                        </ul>
                                    </div>
                                )}
                            </div>
                        </li>
                    </ul>
                </div>

                <div className="flex items-center space-x-3">
                    <button
                        onClick={() => setProfileMenu(!profileMenu)}
                        className="flex text-sm rounded-full bg-gray-200"
                    >
                        <div className="flex items-center justify-center w-10 h-10 bg-gray-100 rounded-full">
                            <span className="font-medium text-gray-600">YL</span>
                        </div>
                    </button>

                    {profileMenu && (
                        <div className="absolute right-0 top-full mt-2 bg-white border border-gray-200 rounded shadow-md w-48 z-50">
                            <div className="px-4 py-3">
                                <span className="block text-sm text-gray-900">{user?.name}</span>
                                <span className="block text-sm text-gray-500 truncate">{user?.email}</span>
                            </div>
                            <ul className="py-2">
                                <li>
                                    <a href="#" className="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100">
                                        Settings
                                    </a>
                                </li>
                                <li>
                                    <NinjaButton
                                        label="Sign out"
                                        onClick={signOut}
                                        className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
                                    />
                                </li>
                            </ul>
                        </div>
                    )}
                </div>
            </div>
        </nav>
    );
};
