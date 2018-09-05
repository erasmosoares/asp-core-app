import { Http } from '@angular/http';
import { Injectable } from '@angular/core';

@Injectable()
export class PhotoService {

  constructor(private http: Http) { }

  upload(vehicleId, photo){
    //'/api/vehicles/'+vehicleId+'/photos'
    var formDate = new FormData();
    formDate.append('file', photo); //must have the same names as parameter controller name
    return this.http.post(`/api/vehicles/${vehicleId}/photos`,formDate)
    .map(res => res.json());
  }

}
