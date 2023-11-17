import { Link } from "react-router-dom";

const Home = () => (
    <div>
        <h1>Home</h1>
        <Link to={`${process.env.REACT_APP_BACKEND_URL}/swagger/index.html`} target="_blank">Api documentation</Link>
    </div>
)

export default Home;
