namespace EloRateApi.Model


open CQAgent

type CreatePlayerMessage = {
    Name: string
    Points: int
    Retired: bool
    }

type PlayerResource = {
  Id : int
  Name : string
  Points : int
  Retired : bool
}


type CreateGameMessage = {
    Winner: string
    Loser: string
    }

type GameResource = {
  Id: int
  Winner : string
  Loser : string
}


type State = { Players: PlayerResource list; Games: GameResource list }  

module Model =

    let initialState = { Players = []; Games = [] }



[<AutoOpen>]
module Player =
    let NewPlayer id (message: CreatePlayerMessage): PlayerResource =
        {
        Id = id
        Name = message.Name
        Points = message.Points
        Retired = message.Retired
        }

    let getPlayerById id = Seq.tryFind (fun (p: PlayerResource) -> p.Id = id)

    let removePlayer id = List.filter (fun (p: PlayerResource) -> p.Id <> id)



    let public GetAll (model: CQAgent<State>) () =
        model.Query (fun (s: State) -> s.Players)

    let public GetItem (model: CQAgent<State>) id =
        model.Query (fun (s: State) -> getPlayerById id s.Players)

    let public Create (model: CQAgent<State>) (message: CreatePlayerMessage) =
        model.Command (fun (s: State) ->
            let newPlayer = (NewPlayer (1 + List.length s.Players) message) 
            (newPlayer, {s with Players = newPlayer::s.Players}))

    let public DeleteItem (model: CQAgent<State>) id =
        model.Command (fun (s: State) ->
            ((), { s with Players = removePlayer id s.Players }))

    let public Update (model: CQAgent<State>) (updatedPlayer: PlayerResource) =
        model.Command (fun (s: State) ->
            let existing = getPlayerById updatedPlayer.Id s.Players
            match existing with
            | Some e -> (Some updatedPlayer, {s with Players = updatedPlayer::removePlayer updatedPlayer.Id s.Players })
            | None -> (None, s))

    let public UpdateById (model: CQAgent<State>) id (message: CreatePlayerMessage) =
        model.Command (fun (s: State) ->
            let existing = getPlayerById id s.Players
            match existing with
            | Some e ->
                    let newPlayer = NewPlayer e.Id message 
                    (Some newPlayer, {s with Players = newPlayer::removePlayer e.Id s.Players })
            | None -> (None, s))





[<AutoOpen>]
module Game =
    let NewGame id (message: CreateGameMessage): GameResource =
        {
        Id = id
        Winner = message.Winner
        Loser = message.Loser
        }

    let getGameById id = Seq.tryFind (fun (g: GameResource) -> g.Id = id)

    let removeGame id = List.filter (fun (g: GameResource) -> g.Id <> id)



    let public GetAll (model: CQAgent<State>) () =
        model.Query (fun (s: State) -> s.Games)

    let public GetItem (model: CQAgent<State>) id =
        model.Query (fun (s: State) -> getGameById id s.Games)

    let public Create (model: CQAgent<State>) (message: CreateGameMessage) =
        model.Command (fun (s: State) ->
            let newGame = (NewGame (1 + List.length s.Games) message) 
            (newGame, {s with Games = newGame::s.Games}))

    let public DeleteItem (model: CQAgent<State>) id =
        model.Command (fun (s: State) ->
            ((), { s with Games = removeGame id s.Games }))

