export class CardiologistModel {
  id: number;
  doctorId: string;
  firstName: string;
  lastName: string;
  degree: string;
  street: string;
  zipCode: string;
  cityID: string;
  phone: string;
  email: string;
  createdBy: string;
  createdOn: Date;
  updatedBy: string;
  updatedOn: Date;
  role: string = 'Cardiologist';
}
