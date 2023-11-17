import { useContext, useEffect, useState } from "react";
import { VerifyUsersService } from "../../services/admin/VerifyUsersService";
import { JwtContext } from "../Root";
import { isUserAdmin } from "../../helpers/IdentityHelpers";
import Error from "../../components/Error";
import { IVerifyUser } from "../../domain/IVerifyUser";
import "../../styles/users.css"

const verifyUsersService = new VerifyUsersService();
const VerifyUsers = () => {
    const { jwtResponse, setJwtResponse } = useContext(JwtContext);
    const [ users, setUsers ] = useState([] as IVerifyUser[]);
    verifyUsersService.initJwtResponse(jwtResponse, setJwtResponse);


    useEffect(() => {
        let isCancelled = false;
        const fetchUsers = async () => {
            const fetchedUsers = await verifyUsersService.getAll();
            if (fetchedUsers && !isCancelled) {
                setUsers(fetchedUsers);
            }
        };
        fetchUsers();
        return () => {
            isCancelled = true;
        }
    }, []);


    if (!jwtResponse || !isUserAdmin(jwtResponse)) {
        return <Error errorMessage={"Access denied!"}></Error>
    }


    return (
        <div className="text-center">
            <h1 className="user-header">Verify users</h1>
            <div className="user-flex-center">
                <div>
            {
                users.map((user) => {
                    return <UserItem key={user.email} user={user} />
                })
            }
            </div>
            </div>
        </div>
    );
}

const UserItem = (props: {user: IVerifyUser}) => {
    const [isVerified, setIsVerified] = useState(props.user.isVerified);
    const [isDisabled, setIsDisabled] = useState(false);

    const handleClick = async () => {
        setIsDisabled(true);

        const user : IVerifyUser = {
            email: props.user.email,
            isVerified: !isVerified
        } 
        const res = await verifyUsersService.set(user);
        if (res) {
            setIsVerified(!isVerified);
        }
        setIsDisabled(false);
    };

    return (
        <div className="user-row text-left">
            <div>{props.user.email}</div>
            <div className="form-check form-switch">
                <input className="form-check-input" type="checkbox" role="switch" id="switchButton" 
                    checked={isVerified} disabled={isDisabled}
                    onChange={handleClick}/>
            </div>

        </div>
    );
}

export default VerifyUsers;