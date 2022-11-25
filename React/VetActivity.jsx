import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Col, Row, Card, ListGroup, Image } from "react-bootstrap";
import PropTypes from "prop-types";
import { formatDate } from "utils/dateFormater";
import debug from "sabio-debug";
import toastr from "toastr";
import vetProfilesService from "../../vetprofile/vetProfilesService";

const _logger = debug.extend("VetActivity");

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

const VetActivity = ({ title }) => {
  const [vetData, setVetsData] = useState({
    vetsArray: [],
    pageIndex: 0,
    pageSize: 5,
    totalCount: 0,
  });

  useEffect(() => {
    vetProfilesService
      .getAllPaginated(vetData.pageIndex, vetData.pageSize)
      .then(onGetVetsSuccess)
      .catch(onGetVetsError);
  }, [vetData.pageIndex, vetData.PageSize]);

  const onGetVetsSuccess = (response) => {
    _logger("GetVetsSucces", response);
    setVetsData((prevState) => {
      const pd = { ...prevState };
      pd.vetsArray = response.item.pagedItems;
      pd.totalCount = response.item.totalCount;
      return pd;
    });
  };

  const onGetVetsError = (error) => {
    _logger("GetVetsError", error);
    toastr.clear();
    toastr.error("Error");
  };

  const mapVetData = (item, index) => (
    <ListGroup.Item
      className={`px-0 ${index === 0 ? "pt-0" : ""}`}
      key={"vet_" + index}
    >
      <Row>
        <Col className="col-auto">
          <div className="avatar avatar-md">
            <Link to={`/vetprofiles/${item.id}`}>
              <Image
                alt="avatar"
                src={item.createdBy.userImage}
                className="rounded-circle"
              />
            </Link>
          </div>
        </Col>
        <Col className="ms-n3">
          <Link to={`/vetprofiles/${item.id}`}>
            <h4 className="mb-0 h5">{`${item.createdBy.firstName} ${item.createdBy.lastName}`}</h4>
          </Link>
          <span className="me-2 fs-6">
            Services{" "}
            <span className="text-dark  me-1 fw-semi-bold">
              {item.serviceTypes.name}
            </span>
          </span>
          <Col className="ps-0">
            <span className="fs-6">
              Created{" "}
              <span className="text-dark  me-1 fw-semi-bold">
                {formatDate(item.dateCreated)}
              </span>
            </span>
          </Col>
        </Col>
      </Row>
    </ListGroup.Item>
  );

  return (
    <Card className="flex">
      <Card.Header className="d-flex align-items-center justify-content-between card-header-height">
        <h4 className="mb-0">{title}</h4>
        <Link to="/vetprofiles" className="btn btn-outline-white btn-sm">
          View all
        </Link>
      </Card.Header>
      <Card.Body>
        <ListGroup variant="flush">
          {vetData.vetsArray.slice(0).reverse().map(mapVetData)}
        </ListGroup>
      </Card.Body>
    </Card>
  );
};
export default VetActivity;

VetActivity.propTypes = {
  title: PropTypes.string.isRequired,
};
