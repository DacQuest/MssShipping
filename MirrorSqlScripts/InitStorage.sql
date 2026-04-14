USE [MssShippingMirrors]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

declare @nodeIndex int
declare @crane int
declare @side int
declare @horz int
declare @vert int
declare @maxCrane int
declare @maxSide int
declare @maxHorz int
declare @maxVert int
declare @binSize int
declare @location int

set @maxCrane = 4
set @maxSide = 2
set @maxHorz = 22
set @maxVert = 6


set @crane = 1
while (@crane <= @maxCrane)
begin
  set @side = 1
  while (@side <= @maxSide)
  begin
    set @horz = 1
    while (@horz <= @maxHorz)
    begin
      set @vert = 1
      while (@vert <= @maxVert)
      begin
        set @nodeIndex = ((@horz - 1) * (@maxVert * 2)) + ((@vert - 1) * 2) + @side - 1 + ((@crane - 1) * (@maxHorz * @maxVert * @maxSide));
        set @location = (@crane * 10000) + (@side * 1000) + (@horz * 10) + @vert
        if ((@crane = 1 or @crane = 3) and (@side = 1) and (@horz = 7 or @horz = 17)) -- NOT USABLE
        begin
          update [Storage]
            set [BinSize]=0, [NotUsable]=1, [BinStatus]=0, [Location]=@location
            where [NodeIndex]=@nodeIndex
        end
        else
        begin
          if (@vert = 1 or @vert = 3 or @vert = 5)
          begin
            set @binSize = 1
          end
          else
          begin
            set @binSize = 2
          end
          update [Storage]
            set [BinSize]=@binSize, [Location]=@location
            where [NodeIndex]=@nodeIndex
        end
        set @vert = @vert + 1
      end
      set @horz = @horz + 1
    end
    set @side = @side + 1
  end
  set @crane = @crane + 1
end

