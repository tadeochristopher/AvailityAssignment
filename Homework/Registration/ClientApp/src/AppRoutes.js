import { Counter } from "./components/Counter";
import { FetchData } from "./components/FetchData";
import { Registration } from "./components/Registration";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/counter',
    element: <Counter />
  },
  {
    path: '/fetch-data',
    element: <FetchData />
  },
  {
    path: '/register',
    element: <Registration />
  }
];

export default AppRoutes;
