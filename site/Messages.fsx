namespace EloRateApi.Messages

    type CreatePlayerMessage = {
        Name: string
        Points: int
        Retired: bool
        }

    type CreateGameMessage = {
        Winner: int
        Loser: int
        }
