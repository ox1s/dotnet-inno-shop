import "./App.css";
import { HeaderNavigationBase } from "@/components/application/app-navigation/header-navigation";

function App() {
  const items = [
    { label: "Home", href: "/" },
    { label: "Catalog", href: "/catalog" },
    { label: "Products", href: "/products" },
    { label: "Account", href: "/account" },
  ];

  return (
    <>
      <HeaderNavigationBase items={items}/>
    </>
  );
}

export default App;
