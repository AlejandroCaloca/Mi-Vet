import React, { useEffect, useState } from "react";
import { Col, Row, Card, ListGroup, Image } from "react-bootstrap";
import PropTypes from "prop-types";
import { Link } from "react-router-dom";
import * as appointmentService from "../../../services/appointmentService";
import { formatDate } from "utils/dateFormater";
import toastr from "toastr";
import debug from "sabio-debug";

const _logger = debug.extend("AppointmentActivity");

toastr.options = {
  toastClass: "analytics-toast-class",
  closeButton: false,
  debug: false,
  newestOnTop: true,
  progressBar: false,
  positionClass: "toast-top-right",
  preventDuplicates: true,
  onclick: null,
  showEasing: "swing",
  hideEasing: "linear",
  showMethod: "fadeIn",
  hideMethod: "fadeOut",
};

const AppointmentActivity = ({ title }) => {
  const [appointmentsData, setAppointmentsData] = useState({
    appointmentsArray: [],
    pageIndex: 0,
    pageSize: 5,
    totalCount: 0,
  });

  useEffect(() => {
    appointmentService
      .getAllAppointments(appointmentsData.pageIndex, appointmentsData.pageSize)
      .then(onGetAppointmentsSuccess)
      .catch(onGetAppointmentsError);
  }, [appointmentsData.pageIndex, appointmentsData.pageSize]);

  const onGetAppointmentsSuccess = (response) => {
    _logger("GetAppointmentsSuccess response", response);
    setAppointmentsData((prevState) => {
      const pd = { ...prevState };
      pd.appointmentsArray = response.item.pagedItems;
      pd.totalCount = response.item.totalCount;
      return pd;
    });
  };

  const onGetAppointmentsError = (error) => {
    _logger("GetAppointmentsError", error);
    toastr.clear();
    toastr.error("Failed to load Appointments");
  };

  const mapAppointments = (item, index) => (
    <ListGroup.Item className="px-0 pt-0 mb-2" key={"appointment_" + index}>
      <Row>
        <Col className="col-auto">
          <Link to={`/owner/patient/${item.patient.id}/view`}>
            <Image
              alt=""
              src={item.patient.primaryImageUrl}
              className="img-fluid rounded img-4by3-sm"
            />
          </Link>
        </Col>
        <Col className="ps-0">
          <Link to={`/appointments`}>
            <h4 className="text-primary-hover">
              Client: {`${item.client.firstName} ${item.client.lastName}`}
            </h4>
          </Link>

          <Link to={`/vetprofiles/${item.vet.id}`}>
            <h5 className="text-primary-hover">
              Veterinerian:{" "}
              {`${item.vet.createdBy.firstName} ${item.vet.createdBy.lastName}`}
            </h5>
          </Link>
          <div>
            <h5 className="mb-1">Status: {item.statusType.name}</h5>
          </div>
          <span className="fs-6">{formatDate(item.dateCreated)}</span>
        </Col>
      </Row>
    </ListGroup.Item>
  );

  return (
    <Card className="flex">
      <Card.Header className="d-flex align-items-center justify-content-between card-header-height">
        <h4 className="mb-0">{title}</h4>
        <Link to="/appointments" className="btn btn-outline-white btn-sm">
          View all
        </Link>
      </Card.Header>
      <Card.Body>
        <ListGroup variant="flush">
          {appointmentsData.appointmentsArray
            .slice(0)
            .reverse()
            .map(mapAppointments)}
        </ListGroup>
      </Card.Body>
    </Card>
  );
};
export default AppointmentActivity;

AppointmentActivity.propTypes = {
  title: PropTypes.string.isRequired,
};
