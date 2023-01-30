import { useState, useEffect } from "react"
import { getBikes } from "../bikeManager"
import BikeCard from "./BikeCard"

export default function BikeList({ setDetailsBikeId }) {
    const [bikes, setBikes] = useState([])

    const getAllBikes = () => {
        getBikes().then(setBikes)
    }

    useEffect(() => {
        getAllBikes()
    }, [])
    return (
        <>
            <h2>Bikes</h2>
            {
                bikes.map(bike => <BikeCard
                    key={`bike--${bike.id}`}
                    bike={bike}
                    setDetailsBikeId={setDetailsBikeId}
                />)
            }
        </>
    )
}