import 'jquery';
import 'popper.js';
import 'bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'font-awesome/css/font-awesome.min.css';

import './index.css';

import React from 'react';
import ReactDOM from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router-dom';

import Root from './routes/Root';
import ErrorPage from './routes/ErrorPage';
import Register from './routes/identity/Register';
import Login from './routes/identity/Login';
import Info from './routes/identity/Info';
import Airports from './routes/Airports';
import Home from './routes/Home';
import Flight from './routes/Flight';
import Reviews from './routes/Reviews';
import Airport from './routes/Airport';
import Review from './routes/Review';
import Flights from './routes/Flights';
import UserFlights from './routes/UserFlights';
import VerifyUsers from './routes/admin/VerifyUsers';
import AirportStatistics from './routes/AirportStatistics';
import UserFlightNotifications from './routes/UserFlightNotifications';
import UserFlightsStatistics from './routes/UserFlightsStatistics';



const router = createBrowserRouter([
  {
      path: "/",
      element: <Root />,
      errorElement: <ErrorPage />,
      children: [
          {
            path: "/",
            element: <Home />
          },
          {
              path: "login/",
              element: <Login />
          },
          {
              path: "register/",
              element: <Register />
          },
          {
              path: "info/",
              element: <Info />
          },
          {
            path: "airports/",
            element: <Airports />
          },
          {
            path: "flights/",
            element: <UserFlights />
          },
          {
            path: "flights/statistics/",
            element: <UserFlightsStatistics />
          },
          {
            path: "airports/:airportIata/",
            element: <Airport />
          },
          {
            path: "airports/:airportIata/departures/",
            element: <Flights isDeparture={true} />
          },
          {
            path: "airports/:airportIata/arrivals/",
            element: <Flights isDeparture={false} />
          },
          {
            path: "airports/:airportIata/reviews/",
            element: <Reviews />
          },
          {
            path: "airports/:airportIata/reviews/edit/:reviewId?/",
            element: <Review />
          },
          {
            path: "airports/:airportIata/departures/flight/:flightId/",
            element: <Flight isDeparture={true} />
          },
          {
            path: "airports/:airportIata/arrivals/flight/:flightId/",
            element: <Flight isDeparture={false} />
          },
          {
            path: "airports/:airportIata/statistics/",
            element: <AirportStatistics />
          },
          {
            path: "flights/flight/:flightId/",
            element: <Flight isDeparture={false} />
          },
          {
            path: "flights/edit/:flightId/",
            element: <UserFlightNotifications />
          },
          {
            path: "admin/users",
            element: <VerifyUsers />
          },
      ]
  },
],
{
  basename: `${process.env.PUBLIC_URL}`,
});



const root = ReactDOM.createRoot(
  document.getElementById('root') as HTMLElement
);
root.render(
  <React.StrictMode>
    <RouterProvider router={router}/>
  </React.StrictMode>
);