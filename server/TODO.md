## Important
- [X] Add unit tests
- [X] Add integration tests
- [X] Delete hard coded admin jwt in account controller
- [X] Replace browser path with proxy browser key in webScraper


## Others
- [X] Create BLL - call flightRepo and flightDataCollector from there. Also do estimated times calculations, flight flown etc.
- [X] Add documentation to public DTOs
- [X] Flight flown - improve (currently does not calculate when status not live, also should also use minutes from last update multiplied by speed)
- [X] Add user flight notifications
- [X] Add user flight notifications - send email
- [X] Add user flights statistics
- [X] Host app in azure
- [X] Replace airlabs api key in code
- [X] Add time period ALL (0) to airport stats
- [X] Add co2 emissions to flight details (comparison to car, train, bus, ship)

### Optional
- [X] Optimize airport stats method - very slow currently
- [X] Fix webScraper sometimes does not return data - maybe wait longer for all responses to load? (for some reason hel departures fails 90% of time)
- [ ] Add a way to create flight-app db on first launch, currently have to create it manually
- [ ] Send flight times in response to set valid minutes before/after scale (shouldn't be able to set notification for time that has already happened)
- [ ] Add enums for notification types (nvm, strings in enums are not supported)
- [ ] Fix all TODOs
