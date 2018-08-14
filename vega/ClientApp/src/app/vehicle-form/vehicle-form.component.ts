import { Vehicle } from './../models/vehicle';
import { SaveVehicle } from './../models/SaveVehicle';
import { ToastrService } from 'ngx-toastr';
import { VehicleService } from '../services/vehicle.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '../../../node_modules/@angular/router';
import { Observable } from '../../../node_modules/rxjs';

@Component({
  selector: 'app-vehicle-form',
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.css']
})
export class VehicleFormComponent implements OnInit {
  makes: any[]; 
  models: any[];
  features: any[];
  vehicle: SaveVehicle = {
    id: 0,
    makeId: 0,
    modelId: 0,
    isRegistered: false,
    features: [],
    contact: {
      name: '',
      email: '',
      phone: '',
    }
  };

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private vehicleService: VehicleService, 
    private toastr: ToastrService){

    route.params.subscribe(p => {
      if(p['id'])this.vehicle.id = +p['id'];
    });

  }

  ngOnInit() {

    var sources = [
      this.vehicleService.getMakes(),
      this.vehicleService.getFeatures(),
    ];

    if(this.vehicle.id)
    sources.push(this.vehicleService.getVehicle(this.vehicle.id));

    Observable.forkJoin(sources).subscribe( data => {
      this.makes = data[0];
      this.features = data[1];

      if(this.vehicle.id){
        this.setVehicle(data[2]);
        this.populateModels();
      }
    }, err => {
      if(err.status == 404)
      this.router.navigate(['']);
      this.showInfo("Woops", "Vehicle not found"); // not found page
   });
  }

  private setVehicle(v: Vehicle){
    this.vehicle.id = v.id;
    this.vehicle.makeId = v.make.id;
    this.vehicle.modelId = v.model.id;
    this.vehicle.isRegistered = v.isRegistered;
    this.vehicle.contact = v.contact;
    this.vehicle.features = v.features.map(f => f.id);// _.pluck(v.features, 'id');
   }

  onMakeChange() {
    this.populateModels();
    delete this.vehicle.modelId;
  }

  private populateModels() {
    var selectedMake = this.makes.find(m => m.id == this.vehicle.makeId);
    this.models = selectedMake ? selectedMake.models : [];
  }

  onFeatureToggle(featureId, $event){
    if($event.target.checked)
     this.vehicle.features.push(featureId);
    else{
     var index =  this.vehicle.features.indexOf(featureId);
     this.vehicle.features.splice(index,1);
    }    
  }

  submit(){
    if(this.vehicle.id){
      this.vehicleService.update(this.vehicle)
      .subscribe(x =>{
        this.showSuccess("Success","The vehicle was sucessfully updated.")
      });
    }
    else{
      this.vehicleService.create(this.vehicle)
      .subscribe(x => console.log(x),
      err=>{
        this.showFailure("Error","An unexpected error happened");
      });
    }
   }

  delete(){
     if(confirm("Are you sure?")){
       this.vehicleService.delete(this.vehicle.id)
       .subscribe(x =>{
         this.router.navigate(['']);
         this.showSuccess("Success","The vehicle was sucessfully deleted.")
       });
     }
   }

  showSuccess(title, message) {
    this.toastr.success(title, message, {
      timeOut: 3000,
    });
  }

  showFailure(title, message) {
    this.toastr.error(title, message, {
      timeOut: 3000,
    });
  }

  showInfo(title, message) {
    this.toastr.info(title, message, {
      timeOut: 3000,
    });
  }
}
