Readme for mermaid representation of AI in GDD

```mermaid
classDiagram
  class MonoBehaviour

  class PathfinderChooser
  enum PathfinderType

  class Pathfinder
  <<abstract>> Pathfinder

  class AStarPathfinder
  class BFSRangePathfinder
  class HexRangePathfinder
  class StarRangePathfinder
  class LineRangePathfinder
  class OpenStarPathfinder

  class CellData
  class ObservableStack

  class Stack<T>

  class HexagonCell
  class HexagonTabletop
  class TabletopBase
  class TabletopMovement
  
  MonoBehaviour <|-- TabletopCamera


  MonoBehaviour <|-- Structure~T~
  MonoBehaviour <|-- TabletopCamera
  MonoBehaviour <|-- Manager
  MonoBehaviour <|-- AgentStatsController
  MonoBehaviour <|-- Fire
  MonoBehaviour <|-- StateMachineRunner

  Manager <|-- RandomManager
  Manager <|-- DRcHandle
  Manager <|-- DRCrowdManager
  Manager <|-- ExplosionManager



  Structure~T~ --> RcVec3f
  Structure~T~ --> ISeedRandom
  Structure~T~ --> List~T~

  Stage --|> Structure~Stage~
  Stage --> DRcHandle

  GreenSpace --|> Structure~GreenSpace~
  GreenSpace --> DRcHandle
  
  FoodArea --|> Structure~FoodArea~
  FoodArea --> DRcHandle
```
