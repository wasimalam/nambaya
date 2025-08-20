export class PharmacyModel {
  id: number;
  name: string;
  email: string;
  phone: string;
  city: string;
  street: string;
  address: string = '';
  zipCode: string;
  createdBy: string;
  updatedBy: string;
  createdOn: Date;
  updatedOn: Date;
  role: string = 'Pharmacy';
  isActive: boolean;
}

// userAccountsContext.createdOn = '2020-02-18T14:12:44.027';
//     userAccountsContext.createdBy = 'system';
//     userAccountsContext.updatedOn = '2020-02-18T14:12:44.027';
//     userAccountsContext.updatedBy = 'system';
