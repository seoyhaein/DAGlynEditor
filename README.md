# DAGlynEditor

## 진행 사항
- OverlayLayer 로 Canvas 구현하였음. 추가 기능 추가 필요 고민할 필요 있음.  
- ItemsControl 연결 해서 위치 정확히 렌더링 되는지 확인 필요.  
- 테스트 코드 삭제 필요 및 추가 기능 구현 세부화 할 필요 있음.  
- ViewPort 구현해야함.  
~~- Panning 구현 진행중 (10-18-23)~~  
~~- Paning 관련 버그 발견. 해결하지 못함. (10-24-23)~~    
- RX 방식으로 이벤트 처리 필요  
~~- 마우스에 따라 Paning 기능 추가 구현 중 (10-22-23)~~    
~~- Animation 구현 (10-19-23)~~  
- EditorCanvas 의 사이즈 설정에 대한 부분 고민해야힘.(최우선을 해결 필요)      
~~- Zoom 기능 추가 구현 필요.~~    
~~- MultiGesture 기능 구현 11/4/23~~    
~~- State 패턴 적용 및 관련 이벤트 적용, MultiGesture 기능 구현 이후. 이때 Zoom, Paning 기능 추가 및 확인. (11/7/23)~~   
- Container 구현 및 Container 에 Node 연결, 그리고, 이때 작업을 편안하게 해주는 API 구현(11/13/23)

## ViewPortLocation 변경 사항 설명
```
마우스 포인터와 관련해서 원본소스에서는 마우스 포인터의 위치를 안에 있는 Panel 로 잡았다. 이걸 다시 상위 Canvas 로 넘겨주는 방식?? 인데

생각을 좀더 해보자. 데이터가 위치할 패널에서 위치를 잡는게 맞는거 같기도 하다.
```
## 미룬 것들 (머리 안써도 되는 것들은 집중안될때 처리)

1. MultiGetsture 테스트 코드 작성 (State 프로젝트 참고)
2. Zoom 기능 추가 구현