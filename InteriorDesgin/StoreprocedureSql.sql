use interior
go

Create Proc [dbo].[CheckingApprovingUserInfo] 
as
begin
 select Z.RoleId , Z.RoleIdCount from ( 
 select  RoleId ,  (count(RoleId))'RoleIdCount' from dbo.UserMasters group by RoleId ) Z where Z.RoleIdCount > 1 ;

 select 
 r.Id , r.Name ,   A.Ir , u.Name 
 from dbo.RoleMasters r  left join  ( 
 select distinct (ias.[Role]) 'Ir' from dbo.InfoApprovedSetups ias
 union 
 select distinct (ias.[NextRole])'Ir' from dbo.InfoApprovedSetups ias 
 )A   
  on A.Ir = r.Name

  left join dbo.UserMasters u

  on r.Id = u.RoleId ;

end

go
CREATE proc [dbo].[LoadExistingSetupByModuleName] (@ModuleName nvarchar(max) ='' , @ItemType nvarchar(max) ='')
 as
 begin

  select B.Id, B.ModuleName ,B.ItemType, B.DepartmentName , B.Location , B.[Role] , B.NextDepartmentName , B.NextLocation , B.NextRole , B.IsActive , B.ApproveSetDate , B.[SysDateTime]  from ( select ias.ModuleName , ias.ItemType , ias.Location , ias.[Role] , ias.NextRole , ias.DepartmentName , ias.NextDepartmentName , ias.NextLocation , max(ias.ApproveSetDate)ApproveSetDate   from dbo.InfoApprovedSetups ias  where ias.IsActive = 1  group by ias.ModuleName , ias.ItemType, ias.Location , ias.[Role] , ias.NextRole , ias.DepartmentName , ias.NextDepartmentName , ias.NextLocation ) A  inner join dbo.InfoApprovedSetups B 
  on A.ModuleName = B.ModuleName 
  and A.ItemType = B.ItemType
  and A.Location = B.Location 
  and A.[Role] = B.[Role] 
  and A.NextRole  = B.NextRole 
  and  A.DepartmentName = B.DepartmentName 
  and  A.NextDepartmentName = B.NextDepartmentName
  and A.ApproveSetDate = B.ApproveSetDate

   where B.ModuleName = @ModuleName and  B.ItemType = @ItemType order by B.Id , B.[SysDateTime] asc

  end
   



   go


  Create Proc [dbo].[UpdateRoleName] (@ToUpdate nvarchar(Max) , @WithUpdate nvarchar(max))
as
begin

    

	update dbo.Departments set DepartmentName  =  RTRIM(  LTRIM( DepartmentName ) ) ;


    update dbo.AspNetRoles set Name  =  RTRIM(  LTRIM( Name ) ) ;

	update dbo.RoleMasters set Name  = RTRIM(  LTRIM( Name  ) ) ;

	update dbo.RoleMenuMappings set RoleIdText  =  RTRIM(  LTRIM( RoleIdText  ) ) ;

	update dbo.InfoApprovedSetups set [Role]  =RTRIM(  LTRIM( [Role]  ) ) ;
	update dbo.InfoApprovedSetups set [NextRole]  =  RTRIM(  LTRIM( [NextRole]  ) ) ;

	update dbo.InfoApprovedInfoes set [Role]  =  RTRIM(  LTRIM( [Role]  ) ) ;



    update dbo.AspNetRoles set Name  = REPLACE(Name , @ToUpdate , @WithUpdate)  where Name like '%'+@ToUpdate ;

	update dbo.RoleMasters set Name  = REPLACE(Name , @ToUpdate , @WithUpdate)  where Name like '%'+@ToUpdate ;

	update dbo.RoleMenuMappings set RoleIdText  = REPLACE(RoleIdText , @ToUpdate , @WithUpdate)  where RoleIdText like '%'+@ToUpdate ;

	update dbo.InfoApprovedSetups set [Role]  = REPLACE([Role] , @ToUpdate , @WithUpdate)  where [Role] like '%'+@ToUpdate ;
	update dbo.InfoApprovedSetups set [NextRole]  = REPLACE([NextRole] , @ToUpdate , @WithUpdate)  where [NextRole] like '%'+@ToUpdate ;

	update dbo.InfoApprovedInfoes set [Role]  = REPLACE([Role] , @ToUpdate , @WithUpdate)  where [Role] like '%'+@ToUpdate ;



	



       
end

go

create proc [dbo].[usp_GetMenuData]
@UserId nvarchar(max)   --user id as input parameter
as
begin                                                                    
                select mm.id , mm.MenuName,mm.MenuURL,mm.MenuParentID from
                UserMasters um                                                       
                inner join RoleMenuMappings rm on um.RoleID=rm.RoleID                                                              
                inner join MenuInfoes mm on mm.Id=rm.MenuInfoId                                                   
                inner join RoleMasters br on br.Id =rm.RoleId                                               
                where um.UserId = @UserId  and rm.Active=1    order by mm.MenuParentId asc ;            -- add more active condition if required                                             
end

go


   CREATE proc [dbo].[usp_MatureInfoReport]( 
			@FromReceiveDate datetime = '',
		    @ToReceiveDate datetime = '')
            as
			begin

			 select bb.* , mi.* , mbd.* , mrd.* from dbo.BankAndBranches bb inner join dbo.MatureInfoes mi 
			 on bb.Id = mi.BankAndBranchId 
			 left join
			 (
			   select  mbd.MatureInfoId , (sum( isnull( mbd.Amount,0.0)))Amount  from dbo.MatureBillInfoDetails mbd group by mbd.MatureInfoId 
			 )mbd 
			 on mi.Id = mbd.MatureInfoId
			 left join
			 (
			   select  mrd.MatureInfoId , (sum(isnull( mrd.ReceiveAmount,0.0)))ReceiveAmount  from dbo.MatureBillReceiveDates mrd group by mrd.MatureInfoId 
			 )mrd 
			 on mi.Id = mrd.MatureInfoId

			 where mi.MatureDate between @FromReceiveDate and @ToReceiveDate

			

		   end 

		   go


create proc [dbo].[usp_ReportHeader]
@Id  int   --user id as input parameter
as
begin                                                                    
              
			   select  * from dbo.ReportSetups where Id =    @Id ;                                         
end ;


