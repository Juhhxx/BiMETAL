# Readme for Mermaid Representation of AI in GDD

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
  class ObservableStack<T>

  class Stack<T>

  class HexagonCell
  class HexagonTabletop
  class TabletopBase
  class TabletopMovement

  class Modifier
  
  Stack<T> <|-- ObservableStack<T>

  Pathfinder <|-- AStarPathfinder
  Pathfinder <|-- BFSRangePathfinder
  Pathfinder <|-- HexRangePathfinder
  Pathfinder <|-- StarRangePathfinder
  Pathfinder <|-- LineRangePathfinder
  Pathfinder <|-- OpenStarPathfinder

  MonoBehaviour <|-- HexagonCell
  MonoBehaviour <|-- HexagonTabletop
  MonoBehaviour <|-- TabletopBase
  TabletopBase <|-- TabletopMovement


  PathfinderChooser ..> Pathfinder
  PathfinderChooser ..> PathfinderType

  Pathfinder --> HexagonTabletop
  Pathfinder --> ObservableStack<T>
  Pathfinder --> MonoBehaviour
  Pathfinder o-- HexagonCell
  Pathfinder *-- CellData
  Pathfinder --> Pathfinder

  CellData --> CellData
  CellData --> HexagonCell

  HexagonCell --> HexagonTabletop
  HexagonCell *-- Modifier
  HexagonCell o-- HexagonCell

  HexagonTabletop o-- HexagonCell

  TabletopBase --> HexagonCell

  TabletopMovement --> Pathfinder
  TabletopMovement ..> HexagonCell
```
