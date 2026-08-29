import eslintPluginPrettierRecommended from "eslint-plugin-prettier/recommended";
import eslintPluginReact from "eslint-plugin-react";
import eslintPluginReactHooks from "eslint-plugin-react-hooks";
import globals from "globals";
import tseslint from "typescript-eslint";

const prettierRules = {
  printWidth: 10000,
  tabWidth: 4,
  useTabs: false,
  semi: true,
  singleQuote: false,
  quoteProps: "as-needed",
  trailingComma: "none",
  bracketSpacing: true,
  arrowParens: "always",
  endOfLine: "lf",
};

export default [
  { ignores: ["eslint.config.mjs", "dist", "node_modules", ".vite"] },
  {
    files: ["**/*.{js,jsx,ts,tsx,mjs,cjs}"],
    ...eslintPluginReact.configs.flat.recommended,
    ...eslintPluginReact.configs.flat["jsx-runtime"],
    plugins: {
      react: eslintPluginReact,
      "react-hooks": eslintPluginReactHooks,
      "@typescript-eslint": tseslint.plugin,
    },
    languageOptions: {
      parser: tseslint.parser,
      globals: {
        ...globals.browser,
        ...globals.node,
      },
      ecmaVersion: "latest",
      sourceType: "module",
      parserOptions: {
        ecmaFeatures: {
          jsx: true,
        },
      },
    },
    settings: {
      react: {
        version: "detect",
      },
    },
    rules: {
      ...tseslint.configs.recommended.rules,
      ...eslintPluginReactHooks.configs.recommended.rules,
      "react-hooks/exhaustive-deps": "off",
      quotes: ["error", "double", { avoidEscape: true }],
      semi: ["error", "always"],
      "comma-dangle": ["error", "never"],
      "max-len": "off",
      "prettier/prettier": ["error", prettierRules],
      "react/react-in-jsx-scope": "off",
    },
  },
  eslintPluginPrettierRecommended,
];
